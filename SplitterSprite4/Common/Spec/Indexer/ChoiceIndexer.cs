// -----------------------------------------------------------------------
// <copyright file="ChoiceIndexer.cs" company="MagicKitchen">
// Copyright (c) MagicKitchen. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MagicKitchen.SplitterSprite4.Common.Spec.Indexer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Indexer class for choosing a choice from several choices accessor.
    /// </summary>
    /// <typeparam name="T">The type of choice.</typeparam>
    public class ChoiceIndexer<T> : LiteralIndexer<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChoiceIndexer{T}"/> class.
        /// </summary>
        /// <param name="parent">The parent spec.</param>
        /// <param name="choices">The choices.</param>
        /// <param name="choiceToSpecStr">The function from choice to string on spec.</param>
        /// <param name="dictMode">This spec is on dictionary value or not.</param>
        internal ChoiceIndexer(
            Spec parent,
            IEnumerable<T> choices,
            Func<T, string> choiceToSpecStr,
            bool dictMode)
            : base(
                  parent,
                  () => $"選択肢({string.Join(", ", choices.Select(choiceToSpecStr))})",
                  (value) =>
                  {
                      return Map(choices, choiceToSpecStr, value);
                  },
                  (value) =>
                  {
                      var key = choiceToSpecStr(value);

                      // the set key must be mappable.
                      Map(choices, choiceToSpecStr, key);

                      return key;
                  },
                  () => $"Choice, {string.Join(", ", choices.Select(choiceToSpecStr))}",
                  MoldingDefault(choices),
                  dictMode)
        {
            var specStrs = new HashSet<string>();
            foreach (var choice in choices)
            {
                var specStr = choiceToSpecStr(choice);

                if (specStrs.Contains(specStr))
                {
                    throw new Spec.InvalidSpecDefinitionException(
                        $"選択肢に対する文字列表現\"{specStr}\"が重複しています。");
                }

                specStrs.Add(specStr);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChoiceIndexer{T}"/> class.
        /// </summary>
        /// <param name="parent">The parent spec.</param>
        /// <param name="choices">The choices.</param>
        /// <param name="dictMode">This spec is on dictionary value or not.</param>
        internal ChoiceIndexer(
            Spec parent,
            IEnumerable<T> choices,
            bool dictMode)
            : this(parent, choices, (choice) => choice.ToString(), dictMode)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChoiceIndexer{T}"/> class.
        /// </summary>
        /// <param name="parent">The parent spec.</param>
        /// <param name="choiceToSpecStr">
        /// The mapping from choice to string on spec.
        /// Note: If the type T's Equals method is not implemented well, this mapping may not work well.
        /// </param>
        /// <param name="dictMode">This spec is on dictionary value or not.</param>
        internal ChoiceIndexer(
            Spec parent,
            IEnumerable<KeyValuePair<T, string>> choiceToSpecStr,
            bool dictMode)
            : this(
                  parent,
                  choiceToSpecStr.Select(kv => kv.Key),
                  (choice) => choiceToSpecStr.Where(kv => kv.Key.Equals(choice)).First().Value,
                  dictMode)
        {
        }

        private static T Map(
            IEnumerable<T> choices,
            Func<T, string> choiceToSpecStr,
            string key)
        {
            foreach (var choice in choices)
            {
                if (key == choiceToSpecStr(choice))
                {
                    return choice;
                }
            }

            throw new Spec.ValidationError();
        }

        private static T MoldingDefault(IEnumerable<T> choices)
        {
            try
            {
                return choices.First();
            }
            catch (ArgumentNullException ex)
            {
                throw new Spec.InvalidSpecDefinitionException(
                    "Choiceアクセスの選択肢がNullです。", ex);
            }
            catch (InvalidOperationException ex)
            {
                throw new Spec.InvalidSpecDefinitionException(
                    "Choiceアクセスの選択肢が空です。", ex);
            }
        }
    }
}
