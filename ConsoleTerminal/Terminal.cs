using BookMarket.DataCollectorLibrary;
using BookMarket.DataLayer.Context;
using BookMarket.DataLayer.Repository;
using BookMarket.DataLayer.Models;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace ConsoleTerminal;

internal class Terminal
{
    public static async Task Main()
    {

    }

    private static async Task TestAuthorCollector()
    {
        AuthorCollector collect = new();
        await collect.CollectFromFantlab();
    }

    private static async Task AddBiographyAndFotoToAuthors()
    {
        AuthorCollector collector = new();
        await collector.AddBiographyAndFoto();
    }

    private static async Task CollectWorksFromFantLab()
    {
        WorkCollector collector = new();
        await collector.CollectWorksFromFantlab();
    }

    private static async Task SetAllAdditionalInfoToWorks(IEnumerable<LiteraryWork> works = null)
    {
        var collector = new WorkCollector();
        await collector.SetJenreAnnotationReviews(works);
    }

    private static async Task TestWorkInsertion(int authorId)
    {
        var lRep = new LiteraryWorkRepository();
        var work = new LiteraryWork()
        {
            Title = "Test",
            AuthorId = authorId,
        };

        await lRep.AddAsync(work);
    }

    private static async Task TestAuthorInsertion()
    {
        var aRep = new AuthorRepository();
        var author = new Author()
        {
            Name = "Pipper Perry",
            BirthDate = "1996-17-06",
            Country = "USA",
            FantlabPage = "of",
        };        

        await aRep.AddRangeAsync(new[] {author });

        Console.WriteLine("Author ID is " + author.Id);
    }

    private static async Task TestAddJenreToWork()
    {
        WorkCollector collector = new();
        await collector.SetJenreInfo();
    }

    private static async Task OzBookCollector(int count)
    {
        BookCollector collector = new();
        using(var ctx = new BookContext())
        {
            var existingWorks = ctx.PageInStore.Include(x => x.Book.LiteraryWorks)
                .Select(w => w.Book.LiteraryWorks.First().Id);
            var works = ctx.Works.Where(w => !existingWorks.Contains(w.Id) && w.isExistToSale).Take(count).Include(w => w.Author);
            await collector.CollectBooksFromOz(works);
        }
        
    }

    private static async Task SetAnnotationTo(int count)
    {
        var workRep = new LiteraryWorkRepository();
        var workCollector = new WorkCollector();

        var works = await workRep.GetAsync(count);
        await workCollector.SetAnnotationFromFL(works);
    }
}

