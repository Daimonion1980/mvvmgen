﻿// ***********************************************************************
// ⚡ MvvmGen => https://github.com/thomasclaudiushuber/mvvmgen
// Copyright © by Thomas Claudius Huber
// Licensed under the MIT license => See LICENSE file in repository root
// ***********************************************************************

using System.Collections.Generic;
using System.Linq;
using MvvmGen.Extensions;
using MvvmGen.Model;
using MvvmGen.SourceGenerators.Generators;

namespace MvvmGen.Generators
{
    internal static class ConstructorGenerator
    {
        internal static void GenerateConstructor(this ViewModelBuilder vmBuilder, ViewModelToGenerate viewModelToGenerate)
        {
            if (viewModelToGenerate.GenerateConstructor)
            {
                Generate(vmBuilder, viewModelToGenerate.ViewModelClassSymbol.Name,
                            viewModelToGenerate.InjectionsToGenerate,
                            viewModelToGenerate.CommandsToGenerate?.Any() == true,
                            viewModelToGenerate.IsEventSubscriber);
            }
        }

        private static void Generate(ViewModelBuilder vmBuilder, string viewModelClassName,
            IEnumerable<InjectionToGenerate>? injectionsToGenerate,
            bool hasCommands, bool isEventSubscriber)
        {
            vmBuilder.AppendLineBeforeMember();
            vmBuilder.Append($"public {viewModelClassName}(");
            injectionsToGenerate ??= Enumerable.Empty<InjectionToGenerate>();

            //var first = true;
            //string? eventAggregatorAccessForSubscription = null;
            //if (isEventSubscriber)
            //{
            //    var eventAggregatorInjection = injectionsToGenerate.FirstOrDefault(x => x.Type == "MvvmGen.Events.IEventAggregator");
            //    if (eventAggregatorInjection is not null)
            //    {
            //        eventAggregatorAccessForSubscription = $"this.{eventAggregatorInjection.PropertyName}";
            //    }
            //    else
            //    {
            //        eventAggregatorAccessForSubscription = "eventAggregator";
            //        first = false;
            //        vmBuilder.Append($"MvvmGen.Events.IEventAggregator {eventAggregatorAccessForSubscription}");
            //    }
            //}
            string? eventAggregatorAccessForSubscription = null;
            var first = vmBuilder.GenerateEventAggregator(injectionsToGenerate,isEventSubscriber, out eventAggregatorAccessForSubscription);

            foreach (var injectionToGenerate in injectionsToGenerate)
            {
                if (!first)
                {
                    vmBuilder.Append(", ");
                }
                first = false;
                vmBuilder.Append($"{injectionToGenerate.Type} {injectionToGenerate.PropertyName.ToCamelCase()}");
            }

            vmBuilder.AppendLine(")");
            vmBuilder.AppendLine("{");
            vmBuilder.IncreaseIndent();
            foreach (var injectionToGenerate in injectionsToGenerate)
            {
                vmBuilder.AppendLine($"this.{injectionToGenerate.PropertyName} = {injectionToGenerate.PropertyName.ToCamelCase()};");
            }

            if (isEventSubscriber)
            {
                vmBuilder.AppendLine($"{eventAggregatorAccessForSubscription}.RegisterSubscriber(this);");
            }

            if (hasCommands)
            {
                vmBuilder.AppendLine($"this.InitializeCommands();");
            }

            vmBuilder.AppendLine($"this.OnInitialize();");
            vmBuilder.DecreaseIndent();
            vmBuilder.AppendLine("}");
            vmBuilder.AppendLine();
            vmBuilder.AppendLine($"partial void OnInitialize();");


        }
    }
}
