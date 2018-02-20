using Swoopie.MemoryManagement;
using System;

namespace Swoopie
{
    public static class ApiFactory
    {
        public static Api Create(AbstractGameProcess gameProcess)
        {
            return new Api(new ProcessMethods(), gameProcess);
        }
    }
}