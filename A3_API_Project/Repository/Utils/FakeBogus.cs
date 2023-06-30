using Bogus;
using A3_API_Project.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace A3_API_Project.Repository.Utils
{
    public class FakeBogus
    {

        private static decimal RandomDecimal(double minimum, double maximum)
        {
            Random random = new Random();
            double valor = random.NextDouble() * (maximum - minimum) + minimum;
            valor = Math.Round(valor, 1);
            return new decimal(valor);
        }

        private static int RandomInt(int min, int max)
        {
            Random random = new Random();
            return random.Next(min, max);
        }

    }
}
