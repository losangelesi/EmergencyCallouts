using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CitizenFX.Core;
using FivePD.API;
using FivePD.API.Utils;
using static CitizenFX.Core.Native.API;

namespace Callouts
{
    [CalloutProperties("Illegally Parked Vehicle", "LosAngelesi", "0.2")]
    public class Illegally_Parked_Car : FivePD.API.Callout
    {
        private readonly Random rnd = new Random();
        private Vehicle vehicle;

        public Illegally_Parked_Car()
        {
            int distance = rnd.Next(300, 750);
            float offsetX = rnd.Next(-1 * distance, distance);
            float offsetY = rnd.Next(-1 * distance, distance);

            InitInfo(World.GetNextPositionOnStreet(Game.PlayerPed.GetOffsetPosition(new Vector3(offsetX, offsetY, 0))));
            ShortName = "Illegally Parked Car";
            CalloutDescription = "Reports of a vehicle parked illegally in the road, respond Code 2";
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
            vehicle = await SpawnVehicle(RandomUtils.GetRandomVehicle(), Location);
            vehicle.AttachBlip();
            vehicle.AttachedBlip.Sprite = (BlipSprite)523;
        }
    }
}
