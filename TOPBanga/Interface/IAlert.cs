using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOPBanga.Interface
{
    public interface IAlert
    {
        bool pathSet { get; }
        void SetPath(String URL);
        void Play();
    }
}
