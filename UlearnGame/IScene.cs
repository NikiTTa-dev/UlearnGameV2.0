﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UlearnGame
{
    public interface IScene
    {
        List<Wall> Walls { get; set; }
    }
}
