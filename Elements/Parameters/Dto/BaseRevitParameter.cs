using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASRR.Revit.Core.Elements.Parameters.Dto
{
    public class BaseRevitParameter<T>
    {
        public string name { get; set; }
        public T value { get; set; }
    }
}
