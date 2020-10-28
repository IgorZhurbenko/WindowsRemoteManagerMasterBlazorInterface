using System;
using System.Collections.Generic;
using System.Text;

namespace Abstract
{
    public interface IUserInteractor
    {
        public Command RecordNewCommand();
        public string RecordNewInstructions();
    }
}
