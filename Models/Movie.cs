using System;
using System.Collections.Generic;
using System.Text;

namespace CinemaApp.Models;

    public  class Movie
    {

        public int MovieId { get; set; }
        public string Title { get; set; }
        public string Genre { get; set; }
        public int DurationMinutes { get; set; }
    }

