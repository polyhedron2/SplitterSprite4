using System;
using System.Collections.Generic;
using System.Text;

namespace MagicKitchen.SplitterSprite4.Common.YAML
{
    public class YAMLTypeSlipException<ExpectedValue> : Exception
        where ExpectedValue : YAML
    {
        public YAMLTypeSlipException(
            string path, YAML value) :
            base($"\"{path}\"の値は{value.GetType().Name}形式であり" +
                 $"期待されている{typeof(ExpectedValue).Name}形式ではありません。")
        { }

        public YAMLTypeSlipException(
            string path, string key, YAML value) :
            this($"{path}[{key}]", value)
        { }
    }

    public class InvalidYAMLStyleException : Exception
    {
        public InvalidYAMLStyleException(string path)
            : base($"YAML\"{path}\"上の形式は想定される形式ではありません。") { }
    }

    public class YAMLKeyUndefinedException : Exception
    {
        public YAMLKeyUndefinedException(string path, string key)
            : base($"YAML\"{path}\"上のキー\"{key}\"は未定義です。") { }
    }
}
