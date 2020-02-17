using UnityEngine;
using System.Collections.Generic;

namespace OneKnight.Loading {
    public interface ArgumentHolder {


        Dictionary<string, object> args { get; set; }
        object[] argValues { get; set; }
    }
}