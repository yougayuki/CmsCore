﻿using CmsCore.Admin.Models;
using CmsCore.Model.Entities;
using CmsCore.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CmsCore.Admin.Controllers
{
    [Authorize(Roles = "ADMIN,SLIDER")]
    public class SlideController : BaseController
    {
        private readonly ISlideService slideService;
        private readonly ISliderService sliderService;
        public SlideController(ISlideService slideService, ISliderService sliderService)
        {
            this.slideService = slideService;
            this.sliderService = sliderService;
        }

        public IActionResult Index()
        {
            var medias = slideService.GetSlides();
            return View(medias);
        }

        public IActionResult Create()
        {
            ViewBag.Slider = new SelectList(sliderService.GetSliders(), "Id", "Name");
            return View();
        }

        [HttpPost]
        public IActionResult Create(SlideViewModel slidevm, IFormFile uploadedFilePhoto, IFormFile uploadedFileVideo)
        {
            if (ModelState.IsValid)
            {

                Slide slide = new Slide();
                slide.Title = slidevm.Title;
                slide.SubTitle = slidevm.SubTitle;
                slide.Description = slidevm.Description;
                slide.CallToActionText = slidevm.CallToActionText;
                slide.CallToActionUrl = slidevm.CallToActionUrl;
                slide.DisplayTexts = slidevm.DisplayTexts;
                slide.SliderId = slidevm.SliderId;
                slide.Position = slideService.CountSlide() + 1;
                slide.Photo = slidevm.Photo;
                slide.Video = slidevm.Video;
                
                slideService.CreateSlide(slide);
                slideService.SaveSlide();
                return RedirectToAction("Index");
            }
            return View(slidevm);
        }

        public IActionResult Edit(long id)
        {
            ViewBag.Slider = new SelectList(sliderService.GetSliders(), "Id", "Name");
            var slide = slideService.GetSlide(id);
            SlideViewModel svm = new SlideViewModel();
            svm.SliderId = slide.SliderId;
            svm.Photo = slide.Photo;
            svm.IsPublished = slide.IsPublished;
            svm.Description = slide.Description;
            svm.CallToActionUrl = slide.CallToActionUrl;
            svm.CallToActionText = slide.CallToActionText;
            svm.Position = slide.Position;
            svm.SubTitle = slide.SubTitle;
            svm.Title = slide.Title;
            svm.Video = slide.Video;
            svm.AddedBy = slide.AddedBy;
            svm.AddedDate = slide.AddedDate;
            svm.ModifiedBy = slide.ModifiedBy;
            svm.ModifiedDate = slide.ModifiedDate;
            svm.DisplayTexts = slide.DisplayTexts;
            ViewBag.FileName = slide.Photo ?? slide.Video;
            return View(svm);
        }

        [HttpPost]
        public IActionResult Edit(SlideViewModel slidevm, IFormFile uploadedFilePhoto, IFormFile uploadedFileVideo)
        {
            ViewBag.Slider = new SelectList(sliderService.GetSliders(), "Id", "Name");
            if (ModelState.IsValid)
            {

                Slide slide = slideService.GetSlide(slidevm.Id);
                slide.SliderId = slide.SliderId;
                slide.Title = slidevm.Title;
                slide.SubTitle = slidevm.SubTitle;
                slide.Description = slidevm.Description;
                slide.CallToActionText = slidevm.CallToActionText;
                slide.CallToActionUrl = slidevm.CallToActionUrl;
                slide.DisplayTexts = slidevm.DisplayTexts;
                slide.SliderId = slidevm.SliderId;
                slide.Position = slideService.CountSlide() + 1;
                slide.AddedBy = slidevm.AddedBy;
                slide.AddedDate = slidevm.AddedDate;
                slide.ModifiedBy = User.Identity.Name ?? "username";
                slide.ModifiedDate = DateTime.Now;
                slide.Photo = slidevm.Photo;
                slide.Video = slidevm.Video;

                

                slideService.UpdateSlide(slide);
                slideService.SaveSlide();
                return RedirectToAction("Index");
            }
            return View(slidevm);
        }
        public IActionResult Delete(long id)
        {
            slideService.DeleteSlide(id);
            slideService.SaveSlide();
            return RedirectToAction("Index", "Slide");
        }
        public IActionResult AjaxHandler(jQueryDataTableParamModel param)
        {
            string sSearch = "";
            if (param.sSearch != null) sSearch = param.sSearch;
            var sortColumnIndex = Convert.ToInt32(Request.Query["iSortCol_0"]);
            var sortDirection = Request.Query["sSortDir_0"]; // asc or desc
            int iTotalRecords;
            int iTotalDisplayRecords;
            var displayedPages = slideService.Search(sSearch, sortColumnIndex, sortDirection, param.iDisplayStart, param.iDisplayLength, out iTotalRecords, out iTotalDisplayRecords);


            var result = from p in displayedPages
                         select new[] {
                             p.Id.ToString(),
                             ("<img src='"+(!String.IsNullOrEmpty(p.Photo)?ViewBag.AssetsUrl+"uploads/media/slide/"+p.Photo.ToString():"")+"' width='100'>"),
                             p.Title.ToString(),
                             (!String.IsNullOrEmpty(p.SubTitle)?p.SubTitle.ToString():""),
                             p.AddedBy.ToString(),
                             p.AddedDate.ToString(),
                             string.Empty };
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = iTotalRecords,
                iTotalDisplayRecords = iTotalDisplayRecords,
                aaData = result.ToList()
            });
        }
    }
}
