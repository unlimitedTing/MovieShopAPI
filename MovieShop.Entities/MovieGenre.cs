﻿using System;
using System.Collections.Generic;
using System.Text;

namespace MovieShop.Entities
{
    public class MovieGenre
    {
        public int MovieId { get; set; }
        public int GenreId { get; set; }
        public virtual Movie Movie { get; set; }
        public virtual Genre Genre { get; set; }

    }
}
