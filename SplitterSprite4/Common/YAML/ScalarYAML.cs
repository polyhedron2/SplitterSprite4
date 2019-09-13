using System;
using System.Collections.Generic;
using System.Text;
using YamlDotNet.RepresentationModel;

namespace MagicKitchen.SplitterSprite4.Common.YAML
{
    // 単一の値を表現するYAMLオブジェクト
    public class ScalarYAML : YAML
    {
        public ScalarYAML(params string[] valueLines)
        {
            Value = string.Join("\n", valueLines);
        }

        public ScalarYAML(string id, YamlScalarNode scalar)
        {
            ID = id;
            if (scalar.Value.EndsWith("\n"))
            {
                // YAMLの複数行入力の場合、末尾に改行が自動挿入されるため削除
                Value = scalar.Value.Substring(0, scalar.Value.Length - 1);
            }
            else
            {
                Value = scalar.Value;
            }
        }

        private string Value { get; set; }

        public bool IsMultiLine
        {
            get => Value.Contains("\n");
        }

        public override IEnumerable<string> ToStringLines()
        {
            // 各行ごとにイテレーション
            return Value.Split(
                new string[] { "\n" }, StringSplitOptions.None);
        }
    }
}
