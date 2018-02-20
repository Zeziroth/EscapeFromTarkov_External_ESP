using UnityExtract.MemoryManagement;
using System;

namespace UnityExtract
{
    public static class ApiFactory
    {
        public static Api Create(AbstractGameProcess gameProcess)
        {
            return new Api(new ProcessMethods(), gameProcess);
        }
    }
}