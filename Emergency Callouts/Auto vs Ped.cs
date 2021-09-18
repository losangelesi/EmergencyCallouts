using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CitizenFX.Core;
using FivePD.API;
using FivePD.API.Utils;
using static CitizenFX.Core.Native.API;

namespace Callouts
{
    [CalloutProperties("Vehicle vs Ped", "LosAngelesi", "0.2")]
    public class Auto_vs_Ped : FivePD.API.Callout
    {
        private readonly Random rnd = new Random();
        private Ped driver, victim;
        private Vehicle vehicle;

        public Auto_vs_Ped()
        {
            int distance = rnd.Next(250, 900);
            float offsetX = rnd.Next(-1 * distance, distance);
            float offsetY = rnd.Next(-1 * distance, distance);

            InitInfo(World.GetNextPositionOnStreet(Game.PlayerPed.GetOffsetPosition(new Vector3(offsetX, offsetY, 0))));
            ShortName = "Auto vs Ped";
            CalloutDescription = "Auto vs Pedestrian, respond Code 3";
            ResponseCode = 3;
            StartDistance = 120f;
        }

        protected void InitBlip(float circleRadius = 75f, BlipColor color = BlipColor.White, BlipSprite sprite = BlipSprite.BigCircle, int alpha = 100)
        {
            Blip blip = World.CreateBlip(this.Location, circleRadius);
            this.Radius = circleRadius;
            this.Marker = blip;
            this.Marker.Sprite = sprite;
            this.Marker.Color = color;
            this.Marker.Alpha = alpha;
        }

        public override async Task OnAccept()
        {
            InitBlip();
            UpdateData();
        }

        public async override void OnStart(Ped player)
        {
            driver = await SpawnPed(RandomUtils.GetRandomPed(), Location);
            victim = await SpawnPed(RandomUtils.GetRandomPed(), Location);
            vehicle = await SpawnVehicle(RandomUtils.GetRandomVehicle(), Location);
            driver.AttachBlip();
            driver.AttachedBlip.Sprite = (BlipSprite)523;
            driver.AttachedBlip.Color = (BlipColor)5;
            driver.SetIntoVehicle(vehicle, VehicleSeat.Driver);
            driver.BlockPermanentEvents = true;
            victim.Kill();
            victim.AttachBlip();
            victim.AttachedBlip.Sprite = (BlipSprite)153;
            victim.AttachedBlip.Color = (BlipColor)57;
            
            var data = await driver.GetData();
            data.BloodAlcoholLevel = (rnd.Next(1, 6) % 5 == 0) ? rnd.NextDouble()*(0.3-0.01)+0.01 : 0;
            data.BloodAlcoholLevel = Math.Round(data.BloodAlcoholLevel, 2);
            driver.SetData(data);
            

            //suspect questions
            PedQuestion question = new PedQuestion();

            question.Question = "What happened?";
            question.Answers = new List<string>
            {
                "They walked out in front of me!",
                "I didn't see them",
                "They were in the way!",
                "I... was speeding..",
                "I'm sorry officer, I don't know",
            };
            AddPedQuestion(driver, question);

            question.Question = "Did you try to slow down?";
            question.Answers = new List<string>
            {
                "Yes",
                "I tried",
                "I think so",
                "I... was speeding..",
                "I'm sorry officer, I don't know",
                "I didn't have time",
                "Nope, roadkill."
            };
            AddPedQuestion(driver, question);

            int chance = rnd.Next(0, 10);
            if (chance >= 0 && chance <= 1)
            {
                TaskVehicleDriveWander(driver.Handle, vehicle.Handle, 45f, 899);
                driver.Task.ReactAndFlee(player);
                question.Question = "Why did you run?";
                question.Answers = new List<string>
                {
                    "Didn't want to get caught",
                    "Bodies scare me",
                    "I was scared of the cops. like you.",
                    "I didn't hit them!",
                    "They faked it!",
                    "I didn't do it",
                    "I didn't? I was driving normally sir.",
                    "Cause i was scared",
                    "Trying to get my car fixed dude!",
                    "Fuck off",
                    "That was my ex. they're fine.",
                    "Why do you care?",
                    "I don't know maaaaaaaaaaaaan"
                };
                AddPedQuestion(driver, question);
            };
        }
    }
}