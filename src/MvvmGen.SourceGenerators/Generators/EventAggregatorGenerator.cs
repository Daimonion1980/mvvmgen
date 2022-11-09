// ***********************************************************************
// ⚡ MvvmGen => https://github.com/thomasclaudiushuber/mvvmgen
// Copyright © by Thomas Claudius Huber
// Licensed under the MIT license => See LICENSE file in repository root
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Xsl;
using MvvmGen.Model;

namespace MvvmGen.SourceGenerators.Generators
{
    internal static class EventAggregatorGenerator
    {
        internal static bool GenerateEventAggregator(this ViewModelBuilder vmBuilder, 
                                                    IEnumerable<InjectionToGenerate>? injectionsToGenerate, 
                                                    bool isEventSubscriber,
                                                    out string? eventAggregatorAccessForSubscription)
        {
            injectionsToGenerate ??= Enumerable.Empty<InjectionToGenerate>();

            var first = true;
            eventAggregatorAccessForSubscription = null;
            if (isEventSubscriber)
            {
                var eventAggregatorInjection = injectionsToGenerate.FirstOrDefault(x => x.Type == "MvvmGen.Events.IEventAggregator");
                if (eventAggregatorInjection is not null)
                {
                    eventAggregatorAccessForSubscription = $"this.{eventAggregatorInjection.PropertyName}";
                }
                else
                {
                    eventAggregatorAccessForSubscription = "eventAggregator";
                    first = false;
                    vmBuilder.Append($"MvvmGen.Events.IEventAggregator {eventAggregatorAccessForSubscription}");
                }
            }
            return first;
        }
    }
}
