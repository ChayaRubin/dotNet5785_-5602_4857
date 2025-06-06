using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PL;
internal class SemestersCollection : IEnumerable
{
    static readonly IEnumerable<BO.CallTypeEnum> s_enums =
(Enum.GetValues(typeof(BO.CallTypeEnum)) as IEnumerable<BO.CallTypeEnum>)!;

    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}

