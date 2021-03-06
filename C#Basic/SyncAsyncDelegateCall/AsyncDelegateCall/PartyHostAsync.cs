﻿using System;
using System.Threading;

namespace AsyncDelegateCall
{
    public delegate string OrderHandle(string s);
    class PartyHostAsync
    {
        public string Name { get; private set; }

        public PartyHostAsync(string name)
        {
            this.Name = name;
        }

        public void CleanUpPlace()
        {
            Console.WriteLine("Cleaning  started at {0}", DateTime.Now.ToLongTimeString());   //cleaning:
            Thread.Sleep(3000);   //register end:
            Console.WriteLine("Cleaning  finished at {0}\n", DateTime.Now.ToLongTimeString());
        }

        public void SetupFurniture()
        {
            Console.WriteLine("Furniture setup  started at {0}", DateTime.Now.ToLongTimeString());   //setting up:
            Thread.Sleep(2000);   //register end:
            Console.WriteLine("Furniture setup  finished at {0}\n", DateTime.Now.ToLongTimeString());
        }

        public void TakeBathAndDressUp()
        {
            Console.WriteLine("TakeBathAndDressUp  started at {0}", DateTime.Now.ToLongTimeString());   //having fun:
            Thread.Sleep(2000);   //register end:
            Console.WriteLine("TakeBathAndDressUp  finished at {0}\n", DateTime.Now.ToLongTimeString());
        }

        public OrderHandle GetRestaurantPhoneNumber()
        {
            OrderHandle phone = new OrderHandle(RestaurantAsync.MakeFood);    //find the restaurant's phone number in the phone book
            return phone;
        }
    }
}
