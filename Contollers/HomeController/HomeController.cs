using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http; // FOR USE OF SESSIONS
using Microsoft.AspNetCore.Mvc;
using Dojoachi.Models;

namespace Dojoachi
{
    public class HomeController: Controller
    {
        [HttpGet("")] //This renders the Main Page        
        public ViewResult Index()
        {
            return View("Index");
        }

        [HttpPost("createDachi")]
        public IActionResult Create(string Name)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Is This Getting Here?");
            string NewName = Name;
            HttpContext.Session.SetString("BuddyName", NewName);
            Console.WriteLine($"###########   NEW BUDDY ---{NewName}--- WAS CREATED  #########");
            // do somethng!  maybe insert into db?  then we will redirect
            return RedirectToAction("StartGame");
        }

        [HttpGet("dojodachi")] 
        public ViewResult StartGame()
        {
            Buddy newBuddy = new Buddy();
            ViewBag.playName = HttpContext.Session.GetString("BuddyName");
            Console.WriteLine($"###########  ---{ViewBag.playName}---- WAS PASSED  #########");
            if(HttpContext.Session.GetInt32("fullness") == null)
            {
                HttpContext.Session.SetInt32("fullness", newBuddy.Fullness);
                HttpContext.Session.SetInt32("happiness", newBuddy.Happiness);
                HttpContext.Session.SetInt32("energy", newBuddy.Energy);
                HttpContext.Session.SetInt32("meals", newBuddy.Meals);
            }
            else
            {
                newBuddy.Fullness = (int)HttpContext.Session.GetInt32("fullness");
                newBuddy.Happiness = (int)HttpContext.Session.GetInt32("happiness");
                newBuddy.Energy = (int)HttpContext.Session.GetInt32("energy");
                newBuddy.Meals = (int)HttpContext.Session.GetInt32("meals");
            }

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"####### We added attributes to ---{ViewBag.playName}----");
            return View("IngameInit", newBuddy); 
        }

        [HttpGet("dojodachiplay")] 
        public IActionResult Playgame(){
            Buddy newBuddy = new Buddy();
            if(HttpContext.Session.GetInt32("happiness") == null){
                HttpContext.Session.SetInt32("happiness", newBuddy.Happiness);
                HttpContext.Session.SetInt32("fullness", newBuddy.Fullness);
                HttpContext.Session.SetInt32("energy", newBuddy.Energy);
                HttpContext.Session.SetInt32("meals", newBuddy.Meals);
            }else{
                newBuddy.Happiness = (int)HttpContext.Session.GetInt32("happiness");
                newBuddy.Fullness = (int)HttpContext.Session.GetInt32("fullness");
                newBuddy.Energy = (int)HttpContext.Session.GetInt32("energy");
                newBuddy.Meals = (int)HttpContext.Session.GetInt32("meals");
            }

            if(newBuddy.Happiness > 100 && newBuddy.Fullness > 100 && newBuddy.Energy > 100){
                ViewBag.message = "Congratulations!, You Won!";
                ViewBag.win = true;
            }else if(newBuddy.Happiness <= 0 || newBuddy.Fullness <= 0){
                ViewBag.message = "Your Dojodachi has passed away...";
                ViewBag.dead = true;
            }else{
                ViewBag.message = TempData["message"];
            }

            return View("Ingame", newBuddy);
        }

        [HttpPost("Feed")]
        public IActionResult Feed(){
            int meals = (int)HttpContext.Session.GetInt32("meals");
            if(meals > 0){
                int fullness = (int)HttpContext.Session.GetInt32("fullness");
                Random rand = new Random();
                int randNumber = rand.Next(5, 11);
                int chanceHated = rand.Next(5);

                if(chanceHated != 1){
                    fullness += randNumber;
                    TempData["message"] = $"Consumed one meal to gain {randNumber} fullness";
                }else{
                    TempData["message"] = "Your dachi did not like that...";
                    TempData["bad"] = true;
                }

                meals--;

                HttpContext.Session.SetInt32("meals", meals);
                HttpContext.Session.SetInt32("fullness", fullness);

                TempData["message"] = $"Consumed one meal to gain {randNumber} fullness";
            }else{
                TempData["message"] = "You do not have any meals left.";
                TempData["bad"] = true;
            }

            return RedirectToAction("Playgame");
        }

        [HttpPost("/Play")]
        public IActionResult Play(){
            int energy = (int)HttpContext.Session.GetInt32("energy");
            int happiness = (int)HttpContext.Session.GetInt32("happiness");

            if(energy > 4){
                Random rand = new Random();
                int randNumber = rand.Next(5, 11);
                int chanceHated = rand.Next(1, 5);

                if(chanceHated != 1){
                    happiness += randNumber;
                    TempData["message"] = $"Played with your dachi and gained {randNumber} happiness, but it cost 5 energy";
                }else{
                    Console.WriteLine("Hated");
                    TempData["message"] = "Your dachi did not like that...";
                    TempData["bad"] = "bad";
                }

                energy -= 5;

                HttpContext.Session.SetInt32("energy", energy);
                HttpContext.Session.SetInt32("happiness", happiness);
            }else{
                TempData["message"] = "Your dachi does not have enough energy to play";
                TempData["bad"] = "bad";
            }

            return RedirectToAction("Playgame");
        }

        [HttpPost("/Work")]   
        public IActionResult Work(){
            int energy = (int)HttpContext.Session.GetInt32("energy");
            int meals = (int)HttpContext.Session.GetInt32("meals");

            if(energy > 4){
                Random rand = new Random();
                int newMeals = rand.Next(1, 4);

                energy -= 5;
                meals += newMeals;

                HttpContext.Session.SetInt32("energy", energy);
                HttpContext.Session.SetInt32("meals", meals);

                TempData["message"] = $"Working cost your dachi 5 energy but it gained {newMeals} meals";
            }else{
                TempData["message"] = "Your dachi does not have enough energy to work";
                TempData["bad"] = "bad";
            }

            return RedirectToAction("Playgame");
        }

        [HttpPost("/Sleep")]
        public IActionResult Sleep(){
            int energy = (int)HttpContext.Session.GetInt32("energy");
            int fullness = (int)HttpContext.Session.GetInt32("fullness");
            int happiness = (int)HttpContext.Session.GetInt32("happiness");

            energy += 15;
            fullness -= 5;
            happiness -= 5;

            TempData["message"] = "Sleeping gave your dachi 15 energy but lowered fullness and happiness by 5";

            HttpContext.Session.SetInt32("energy", energy);
            HttpContext.Session.SetInt32("happiness", happiness);
            HttpContext.Session.SetInt32("fullness", fullness);

            return RedirectToAction("Playgame");
        }

        [HttpPost("/Restart")]
        public IActionResult Restart(){
            HttpContext.Session.Clear();
            TempData["message"] = "";
            return RedirectToAction("Index");
        }
    }
}