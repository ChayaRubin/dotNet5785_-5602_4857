using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PL;
internal class SemestersCollection : IEnumerable
{
    static readonly IEnumerable<BO.CallStatus> s_enums =
(Enum.GetValues(typeof(BO.CallStatus)) as IEnumerable<BO.CallStatus>)!;

    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}

