using System;
using System.Collections.Generic;

namespace Editor
{
    public class CommandLineArgs
    {
        readonly Dictionary<string, string> _arguments = new();

        public CommandLineArgs(string[] args)
        {
            for (var i = 0; i < args.Length; i++)
            {
                if (!args[i].StartsWith("-")) continue;

                // 引数名のみの場合、値を固定で true にする
                if (i + 1 >= args.Length || args[i + 1].StartsWith("-"))
                {
                    _arguments[args[i].TrimStart('-')] = "true";
                    continue;
                }

                // 引数名と値のペアを追加
                _arguments[args[i].TrimStart('-')] = args[i + 1];
                i++;
            }
        }

        public T Get<T>(string name, T defaultValue = default)
        {
            if (!_arguments.TryGetValue(name, out var value))
            {
                return defaultValue;
            }
            try
            {
                return (T)Convert.ChangeType(value, typeof(T));
            }
            catch
            {
                throw new ArgumentException($"引数 '{name}' の値を型 '{typeof(T).Name}' に変換できませんでした: {value}");
            }
        }

        public T GetRequired<T>(string name)
        {
            if (!_arguments.TryGetValue(name, out var value) || string.IsNullOrEmpty(value))
            {
                throw new ArgumentException($"必須引数が指定されていません: {name}");
            }

            try
            {
                return (T)Convert.ChangeType(value, typeof(T));
            }
            catch
            {
                throw new ArgumentException($"引数 '{name}' の値を型 '{typeof(T).Name}' に変換できませんでした: {value}");
            }
        }

        public bool Has(string name)
        {
            return _arguments.ContainsKey(name);
        }
    }
}