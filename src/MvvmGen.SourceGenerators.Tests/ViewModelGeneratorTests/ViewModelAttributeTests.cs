﻿// ***********************************************************************
// ⚡ MvvmGen => https://github.com/thomasclaudiushuber/mvvmgen
// Copyright © by Thomas Claudius Huber
// Licensed under the MIT license => See the LICENSE file in project root
// ***********************************************************************

using Xunit;

namespace MvvmGen.SourceGenerators
{
    public class ViewModelAttributeTests : ViewModelGeneratorTestsBase
    {
        [InlineData("[MvvmGen.ViewModel]")]
        [InlineData("[ViewModel]")]
        [Theory]
        public void GenerateViewModel(string attribute)
        {
            ShouldGenerateExpectedCode(
      $@"using MvvmGen; 

namespace MyCode
{{   
  {attribute}
  public partial class EmployeeViewModel
  {{
  }}
}}",
      $@"{AutoGeneratedComment}
{AutoGeneratedUsings}

namespace MyCode
{{
    partial class EmployeeViewModel : ViewModelBase
    {{
        public EmployeeViewModel()
        {{
            this.OnInitialize();
        }}

        partial void OnInitialize();
    }}
}}
");
        }

        [Fact]
        public void GenerateViewModelWithoutConstructor()
        {
            ShouldGenerateExpectedCode(
      $@"using MvvmGen; 

namespace MyCode
{{   
  [ViewModel(GenerateConstructor=false)]
  public partial class EmployeeViewModel
  {{
  }}
}}",
      $@"{AutoGeneratedComment}
{AutoGeneratedUsings}

namespace MyCode
{{
    partial class EmployeeViewModel : ViewModelBase
    {{
    }}
}}
");
        }

        [Fact]
        public void GenerateViewModelWithoutConstructorNoEmptyLineForFirstMember()
        {
            ShouldGenerateExpectedCode(
      $@"using MvvmGen; 

namespace MyCode
{{  
  [Inject(typeof(string),""InjectedString"")]
  [ViewModel(GenerateConstructor=false)]
  public partial class EmployeeViewModel
  {{
  }}
}}",
      $@"{AutoGeneratedComment}
{AutoGeneratedUsings}

namespace MyCode
{{
    partial class EmployeeViewModel : ViewModelBase
    {{
        protected string InjectedString {{ get; private set; }}
    }}
}}
");
        }

        [Fact]
        public void GenerateViewModelBaseNotIfClassInderictlyInheritsFromIt()
        {
            ShouldGenerateExpectedCode(
       @"using System.ComponentModel;
using MvvmGen;
using MvvmGen.ViewModels;

namespace MyCode
{   
  public class CustomViewModelBase : ViewModelBase
  {
    protected void OnPropertyChanged(string propertyName) { }
  }

  [ViewModel]
  public partial class EmployeeViewModel : CustomViewModelBase
  {
  }
}",
       $@"{AutoGeneratedComment}
{AutoGeneratedUsings}

namespace MyCode
{{
    partial class EmployeeViewModel
    {{
        public EmployeeViewModel()
        {{
            this.OnInitialize();
        }}

        partial void OnInitialize();
    }}
}}
");
        }


        [InlineData("ModelType=typeof(Employee)")]
        [InlineData("typeof(Employee)")]
        [Theory]
        public void GenerateModelProperties(string attributeArgument)
        {
            ShouldGenerateExpectedCode(
      $@"using MvvmGen;

namespace MyCode
{{   
    public class Employee
    {{
        public string FirstName {{ get; set; }}
        public bool IsDeveloper {{ get; set; }}
    }}

    [ViewModel({attributeArgument})]
    public partial class EmployeeViewModel
    {{
    }}
}}",
      $@"{AutoGeneratedComment}
{AutoGeneratedUsings}

namespace MyCode
{{
    partial class EmployeeViewModel : ViewModelBase
    {{
        public EmployeeViewModel()
        {{
            this.OnInitialize();
        }}

        partial void OnInitialize();

        public MyCode.Employee Model {{ get; set; }}

        public string FirstName
        {{
            get => Model.FirstName;
            set
            {{
                if (Model.FirstName != value)
                {{
                    Model.FirstName = value;
                    OnPropertyChanged(""FirstName"");
                }}
            }}
        }}

        public bool IsDeveloper
        {{
            get => Model.IsDeveloper;
            set
            {{
                if (Model.IsDeveloper != value)
                {{
                    Model.IsDeveloper = value;
                    OnPropertyChanged(""IsDeveloper"");
                }}
            }}
        }}
    }}
}}
");
        }
        [InlineData("ModelType=typeof(Employee)")]
        [InlineData("typeof(Employee)")]
        [Theory]
        public void GenerateModelPropertiesWhenReadOnlyPropsExist(string attributeArgument)
        {
            ShouldGenerateExpectedCode(
      $@"using MvvmGen;

namespace MyCode
{{   
    public class Employee
    {{
        public Employee (int id)
        {{
            Id = id;
        }}
        public int Id {{ get; }}
        public bool IsDeveloper {{ get; set; }}
    }}

    [ViewModel({attributeArgument})]
    public partial class EmployeeViewModel
    {{
    }}
}}",
      $@"{AutoGeneratedComment}
{AutoGeneratedUsings}

namespace MyCode
{{
    partial class EmployeeViewModel : ViewModelBase
    {{
        public EmployeeViewModel()
        {{
            this.OnInitialize();
        }}

        partial void OnInitialize();

        public MyCode.Employee Model {{ get; set; }}

        public int Id => Model.Id;

        public bool IsDeveloper
        {{
            get => Model.IsDeveloper;
            set
            {{
                if (Model.IsDeveloper != value)
                {{
                    Model.IsDeveloper = value;
                    OnPropertyChanged(""IsDeveloper"");
                }}
            }}
        }}
    }}
}}
");
        }
    }
}
