using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Dojoachi.Models{
    public class Buddy 
    {
        [Required(ErrorMessage = "Please Enter a Name for Your Dojoachi")]
        [MinLength(4, ErrorMessage = "Name must be at least 4 characters")]

        public int Fullness {get;set;}
        public int Happiness {get;set;}
        public int Meals {get;set;}
        public int Energy {get;set;}

        public Buddy()
        {
            this.Fullness = 20;
            this.Happiness = 20;
            this.Meals = 3;
            this.Energy = 50;
        }

        public Buddy(int happiness, int fullness, int energy, int meals)
        {

            this.Fullness = fullness;
            this.Happiness = happiness;
            this.Meals = meals;
            this.Energy = energy;
        }

    }

}