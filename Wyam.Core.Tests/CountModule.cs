﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wyam.Abstractions;

namespace Wyam.Core.Tests
{
    public class CountModule : IModule
    {
        public int AdditionalOutputs { get; set; }  // Controls how many additional outputs are spawned
        public string ValueKey { get; set; }  // This is the key used for storing the value in the metadata
        public int Value { get; set; }  // This is incremented on every call and output and added to the metadata
        public int ExecuteCount { get; set; }
        public int InputCount { get; set; }
        public int OutputCount { get; set; }

        public CountModule(string valueKey)
        {
            ValueKey = valueKey;
        }

        public IEnumerable<IDocument> Execute(IReadOnlyList<IDocument> inputs, IExecutionContext context)
        {
            ExecuteCount++;
            foreach (IDocument input in inputs)
            {
                InputCount++;
                for (int c = 0; c < AdditionalOutputs + 1; c++)
                {
                    OutputCount++;
                    Value++;
                    yield return input.Clone(input.Content == null ? Value.ToString() : input.Content + Value, 
                        new Dictionary<string, object> { { ValueKey, Value } });
                }
            }
        }
    }
}
