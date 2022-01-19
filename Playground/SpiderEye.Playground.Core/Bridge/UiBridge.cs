using System;
using System.Threading;
using System.Threading.Tasks;

namespace SpiderEye.Playground.Core
{
    public class UiBridge
    {
        private readonly Random random = new();

        public static async Task RunLongProcedureOnTask()
        {
            await Task.Delay(TimeSpan.FromSeconds(10));
        }

        public static void RunLongProcedure()
        {
            Thread.Sleep(TimeSpan.FromSeconds(10));
        }

        public SomeDataModel GetSomeData()
        {
            return new SomeDataModel
            {
                Text = "Hello World",
                Number = random.Next(100),
            };
        }

        public static double Power(PowerModel model)
        {
            return Math.Pow(model.Value, model.Power);
        }

        public static void ProduceError()
        {
            throw new Exception("Intentional exception from .Net");
        }
    }
}
