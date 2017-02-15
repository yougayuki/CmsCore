﻿using CmsCore.Admin.Models;
using CmsCore.Model.Entities;
using CmsCore.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CmsCore.Admin.Controllers
{
    public class MenuItemController : BaseController
    {
        private readonly IMenuService menuService;
        private readonly IMenuLocationService menuLocationService;
        private readonly IMenuItemService menuItemService;
        public MenuItemController(IMenuService menuService, IMenuLocationService menuLocationService,IMenuItemService menuItemService)
        {
            this.menuService = menuService;
            this.menuLocationService = menuLocationService;
            this.menuItemService = menuItemService;
        }
        // GET: /<controller>/
        public IActionResult Index(){
            return View();
        }
        public IActionResult Create(){
            
            ViewBag.ParentMenuItem = new SelectList(menuItemService.GetMenuItems(), "Id", "Name");
            
            return View();
        }
        [HttpPost]
        public IActionResult Create(MenuItem menuItem){
            if (ModelState.IsValid)
            {
                menuItemService.CreateMenuItem(menuItem);
                menuItemService.SaveMenuItem();
                return RedirectToAction("Index", "MenuItem");
            }
            ViewBag.ParentMenuItem = new SelectList(menuItemService.GetMenuItems(), "Id", "Name", menuItem.Id);
            return View(menuItem);

            
        }
        public IActionResult Edit(long id){
            return View();
        }
        [HttpPost]
        public IActionResult Edit(MenuItem menuItem){
            
            return View();
        }
        public IActionResult Delete(long id){
            
            return RedirectToAction("Index", "Menu");
        }
        public IActionResult AjaxHandler(jQueryDataTableParamModel param){
            string sSearch = "";
            if (param.sSearch != null) sSearch = param.sSearch;
            var sortColumnIndex = Convert.ToInt32(Request.Query["iSortCol_0"]);
            var sortDirection = Request.Query["sSortDir_0"]; // asc or desc
            int iTotalRecords;
            int iTotalDisplayRecords;
            var displayedPages = menuItemService.Search(sSearch, sortColumnIndex, sortDirection, param.iDisplayStart, param.iDisplayLength, out iTotalRecords, out iTotalDisplayRecords);

            var result = from p in displayedPages
                         select new[]{
                             p.Id.ToString(),
                             p.Id.ToString(),
                             p.Name,
                             p.Url,
                             p.Target,
                             
                             string.Empty
                         };
            return Json(new{
                sEcho = param.sEcho,
                iTotalRecords = iTotalRecords,
                iTotalDisplayRecords = iTotalDisplayRecords,
                aaData = result.ToList()
            });
        }
    }
}
