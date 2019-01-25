namespace SpiderEye.Json.TestModels
{
    internal class AttributesModel
    {
        public static readonly AttributesModel DefaultWrite = new AttributesModel
        {
            IgnoredProperty = "IgnoredProperty",
            RawProperty = "{\"innerRawPropertyValue\":{\"foo\":[{\"bar\":123},456,\"Hello World\"]},\"baz\":7.89}",
            IgnoredField = "IgnoredField",
            RawField = "{\"innerRawFieldValue\":{\"foo\":[{\"bar\":123},456,\"Hello World\"]},\"baz\":7.89}",
        };

        public static readonly AttributesModel DefaultRead = new AttributesModel
        {
            IgnoredProperty = null,
            RawProperty = "{\"innerRawPropertyValue\":{\"foo\":[{\"bar\":123},456,\"Hello World\"]},\"baz\":7.89}",
            IgnoredField = null,
            RawField = "{\"innerRawFieldValue\":{\"foo\":[{\"bar\":123},456,\"Hello World\"]},\"baz\":7.89}",
        };

        public const string DefaultWriteJson = "{" +
            "\"rawProperty\":{\"innerRawPropertyValue\":{\"foo\":[{\"bar\":123},456,\"Hello World\"]},\"baz\":7.89}," +
            "\"rawField\":{\"innerRawFieldValue\":{\"foo\":[{\"bar\":123},456,\"Hello World\"]},\"baz\":7.89}" +
            "}";

        public const string DefaultReadJson = "{" +
            "\"ignoredProperty\":\"IgnoredProperty\"," +
            "\"rawProperty\":{\"innerRawPropertyValue\":{\"foo\":[{\"bar\":123},456,\"Hello World\"]},\"baz\":7.89}," +
            "\"ignoredField\":\"IgnoredField\"," +
            "\"rawField\":{\"innerRawFieldValue\":{\"foo\":[{\"bar\":123},456,\"Hello World\"]},\"baz\":7.89}" +
            "}";

        [JsonIgnore]
        public string IgnoredProperty { get; set; }
        [RawJson]
        public string RawProperty { get; set; }

        [JsonIgnore]
        public string IgnoredField;
        [RawJson]
        public string RawField;

        public override bool Equals(object obj)
        {
            if (obj is AttributesModel model)
            {
                return IgnoredField == model.IgnoredField
                    && RawField == model.RawField
                    && IgnoredProperty == model.IgnoredProperty
                    && RawProperty == model.RawProperty;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
