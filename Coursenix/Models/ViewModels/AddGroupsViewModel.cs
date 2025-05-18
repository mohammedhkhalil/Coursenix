// Models/ViewModels/AddGroupsViewModel.cs - Corrected with [BindNever]

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Coursenix.Models; // Ensure this using directive is present
using Coursenix.Models.ViewModels; // Ensure this using directive is present
using Microsoft.AspNetCore.Mvc.ModelBinding; // Required for [BindNever]

namespace Coursenix.Models.ViewModels
{
    public class AddGroupsViewModel
    {
        // SubjectId is submitted via a hidden input, so it should be bindable
        public int SubjectId { get; set; }

        // Subject is for display purposes on GET, not for binding on POST.
        // Use [BindNever] to prevent model binder from trying to bind it on POST.
        [BindNever]
        public Subject? Subject { get; set; }

        // This list holds the data for the groups being added/edited, it IS bindable
        public List<GroupViewModel> Groups { get; set; }

        // StatusMessage is for display purposes, not for binding on POST.
        // Use [BindNever] to prevent model binder from trying to bind it on POST.
        [BindNever]
        public string StatusMessage { get; set; }

        // IsSuccess is for display purposes, not for binding on POST.
        // Use [BindNever] to prevent model binder from trying to bind it on POST.
        [BindNever]
        public bool IsSuccess { get; set; } = false;

        // Note: GroupViewModel should be defined in its own file (GroupViewModel.cs)
        // and referenced here.
    }
}