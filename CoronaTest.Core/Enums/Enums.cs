using System;
using System.Collections.Generic;
using System.Text;

namespace CoronaTest.Core
{
    public enum TestResult
    {
        Unknown,
        Positive,
        Negative
    }

    public enum ExaminationStates
    {
        New,
        Registered,
        Tested
    }
}
