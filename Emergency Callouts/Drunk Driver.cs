using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CitizenFX.Core;
using FivePD.API;
using FivePD.API.Utils;
using static CitizenFX.Core.Native.API;

namespace Callouts
{
    [CalloutProperties("DUI", "LosAngelesi", "0.2")]
    public class DUI : FivePD.API.Callout
    {
        private readonly Random rnd = new Random();
        private Ped driver;
        private Vehicle vehicle;

        public DUI()
        {
            int distance = rnd.Next(300, 750);
            float offsetX = rnd.Next(-1 * distance, distance);
            float offsetY = rnd.Next(-1 * distance, distance);

            InitInfo(World.GetNextPositionOnStreet(Game.PlayerPed.GetOffsetPosition(new Vector3(offsetX, offsetY, 0))));
            ShortName = "Poss. DUI";
            CalloutDescription = "Possible Drive under the influence, respond Code 2";
            ResponseCode = 2;
            StartDistance = 120f;
        }

        protected void InitBlip(float circleRadius = 150f, BlipColor color = BlipColor.Blue, BlipSprite sprite = BlipSprite.BigCircle, int alpha = 100)
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
            vehicle = await SpawnVehicle(RandomUtils.GetRandomVehicle(), Location);
            driver.AttachBlip();
            driver.AttachedBlip.Sprite = (BlipSprite)523;
            driver.AttachedBlip.Color = (BlipColor)5;
            driver.SetIntoVehicle(vehicle, VehicleSeat.Driver);
            driver.BlockPermanentEvents = true;
            TaskVehicleDriveWander(driver.Handle, vehicle.Handle, 35f, 899);

            var data = await driver.GetData();
            data.BloodAlcoholLevel = rnd.NextDouble() * (0.3 - 0.01) + 0.01;
            data.BloodAlcoholLevel = Math.Round(data.BloodAlcoholLevel, 2);
            driver.SetData(data);

            //suspect questions
            PedQuestion question = new PedQuestion();

            question.Question = "Do you know why we stopped you?";
            question.Answers = new List<string>
            {
                "Nope",
                "Because you're bored?",
                "I'm a free traveler, you have no right to stop me",
                "Was I speeding?",
                "Not at all",
            };
            AddPedQuestion(driver, question);

            int chance = rnd.Next(0, 10);
            if (chance >= 0 && chance <= 0.5)
            {
                driver.Task.ReactAndFlee(player);
                question.Question = "Why did you run?";
                question.Answers = new List<string>
                {
                    "Didn't want to get caught",
                    "Just enjoying the road",
                    "Fast & Furious style bro!",
                    "I was hungry!",
                    "Just had a drive fast moment, you know?",
                    "None of your business!",
                    "I didn't?",
                    "I wasn't bro",
                    "Trying to get my drive on bro",
                    "Fuck off",
                    "I was just driving bro",
                    "Why do you care?",
                    "I'm not drunk"
                };
                AddPedQuestion(driver, question);
            };
        }
    }
}