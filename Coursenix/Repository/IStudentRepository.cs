using Microsoft.AspNetCore.Mvc;

namespace Coursenix.Repository
{
    public interface IStudentRepository
    {
        IActionResult Create();
        IActionResult Update();
        IActionResult Delete();
        IActionResult GetInfo();

    }
}
