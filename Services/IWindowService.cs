using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warehouse1.Services
{
    public interface IWindowService
    {
        void CloseLoginAndOpen(string role, int userId);
    }
}
