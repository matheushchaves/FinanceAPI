using System;

namespace FinanceAPI.Models.DynamicTable
{
    public class Field
    {
        public int Order { get; set; }
        public bool Key { get; set; } = false ;
        public bool Required { get; set; } = false;
        public string Label { get; set; }
        public bool Visible { get; set; } = true;
        public bool Disabled { get; set; } = false;
        public bool Sortable { get; set; }
        public string Format { get; set; }
        public string Property { get; set; }
        public string Tooltip { get; set; }
        public string Type { get; set; } = "string";
        public bool Filter { get; set; } = false;
        public bool AllowColumnsManager { get; set; } = true;
        public List<dynamic> Options { get; set; } 

        public double MinValue { get; set; }
        public double MaxValue { get; set; }

        public int MinLength { get; set; }
        public int MaxLength { get; set; }

        public string Help { get; set; }

        public string BooleanTrue { get; set; }
        public string BooleanFalse { get; set; }
        public string ErrorMessage { get; set; }
        public string Placeholder { get; set; }



    }


}
