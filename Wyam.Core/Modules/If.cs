﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wyam.Abstractions;

namespace Wyam.Core.Modules
{
    // This executes the specified modules if the specified predicate is true
    // Any results from the specified modules (if run) will be returned as the result of the If module
    // Like a Branch module, but results replace the input documents at the end (instead of being dropped)
    public class If : IModule
    {
        private readonly List<Tuple<Func<IDocument, bool>, IModule[]>> _conditions 
            = new List<Tuple<Func<IDocument, bool>, IModule[]>>();

        public If(Func<IDocument, bool> predicate, params IModule[] modules)
        {
            _conditions.Add(new Tuple<Func<IDocument, bool>, IModule[]>(predicate, modules));
        }

        public If ElseIf(Func<IDocument, bool> predicate, params IModule[] modules)
        {
            _conditions.Add(new Tuple<Func<IDocument, bool>, IModule[]>(predicate, modules));
            return this;
        }

        // Returns IModule instead of If to discourage further conditions
        public IModule Else(params IModule[] modules)
        {
            _conditions.Add(new Tuple<Func<IDocument, bool>, IModule[]>(x => true, modules));
            return this;
        }

        public IEnumerable<IDocument> Execute(IReadOnlyList<IDocument> inputs, IExecutionContext context)
        {
            List<IDocument> results = new List<IDocument>();
            IEnumerable<IDocument> documents = inputs;
            foreach (Tuple<Func<IDocument, bool>, IModule[]> condition in _conditions)
            {
                // Split the documents into ones that satisfy the predicate and ones that don't
                List<IDocument> handled = new List<IDocument>();
                List<IDocument> unhandled = new List<IDocument>();
                foreach (IDocument document in documents)
                {
                    if (condition.Item1 == null || condition.Item1(document))
                    {
                        handled.Add(document);
                    }
                    else
                    {
                        unhandled.Add(document);
                    }
                }

                // Run the modules on the documents that satisfy the predicate
                results.AddRange(context.Execute(condition.Item2, handled));

                // Continue with the documents that don't satisfy the predicate
                documents = unhandled;
            }

            // Add back any documents that never matched a predicate
            results.AddRange(documents);

            return results;
        }
    }
}
