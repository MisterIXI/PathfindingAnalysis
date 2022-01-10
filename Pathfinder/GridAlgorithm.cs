using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pathfinder
{
    interface GridAlgorithm
    {
        void setStepDelay(int stepDelay);
        void drawPath(FieldItem startPoint, FieldItem targetPoint, bool diagAllowed);
    }
}
