namespace SpiderEye.Json.TestModels
{
    internal class EmptyObjectModel
    {
        public override bool Equals(object obj)
        {
            return obj is EmptyObjectModel;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
