using CinemaApp.Data;
using CinemaApp.Models;
using CinemaApp.Repository;
using CinemaApp.Services;
using Microsoft.EntityFrameworkCore;
using System;

public class Program
{
    public static void Main(string[] args)
    {

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlServer(
                @"Server=.\SQLEXPRESS;Database=CinemaAppDB;Trusted_Connection=True;TrustServerCertificate=True;")
            .Options;

        using var db = new AppDbContext(options);

        Console.WriteLine("SeatRush Movie Database Ready!");
        MovieService obj1 = new MovieService();
        bool valid = true;
        while (valid)
        {

            Console.WriteLine("Enter your choice : ");
            string choice = Console.ReadLine();

            switch (choice)
            {

                case "1":
                    obj1.Run();
                    break;

            }
        }
    }
}