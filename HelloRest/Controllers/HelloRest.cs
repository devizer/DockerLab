﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace HelloRest.Controllers
{
    [Route("api/v1")]
    public class HelloRest : Controller
    {
        [HttpPost]
        [Route("status")]
        public object Status()
        {
            return new { OK = true };
        }

        [HttpGet]
        [Route("ping")]
        public JsonResult Ping()
        {
            return Json("Pong");
        }

    }
}
