
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

using Microsoft.EntityFrameworkCore;


using CsvHelper;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace MvcPractice.EF
{
    public class DataSeed
    {
        private class StateComparer : IEqualityComparer<CountryState>
        {
            public bool Equals(CountryState x, CountryState y)
            {
                return x?.Name == y?.Name;
            }

            public int GetHashCode([DisallowNull] CountryState obj)
            {
                return obj.Name.GetHashCode();
            }
        }

        private static List<LGAData> GetLgaData()
        {
            using var reader = new StreamReader("LGA.csv");
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            var records = csv.GetRecords<LGAData>().ToList();
            return records;
        }

        private static async Task SeedCountryState(AppDbContext context)
        {
            var data = GetLgaData();
            var countryStates = data.Select(x => new CountryState { Name = x.State })
                .ToList().Distinct(new StateComparer());
            await context.CountryStates.AddRangeAsync(countryStates);
            await context.SaveChangesAsync();
        }

        private static async Task SeedLga(AppDbContext context)
        {
            var data = GetLgaData();
            var states = context.CountryStates.ToList();

            var lgas = data.Select(x => new Lga
            {
                Name = x.Lga,
                StateId = states.First(y => y.Name == x.State).Id
            });
            await context.Lgas.AddRangeAsync(lgas);
            await context.SaveChangesAsync();
        }

        public static async Task EnsureSeedDataPopulated(IApplicationBuilder app)
        {
            var serviceProvider = app.ApplicationServices.CreateScope().ServiceProvider;
            await using (var context = (AppDbContext)serviceProvider.GetService(typeof(AppDbContext)))
            {
                if (context == null)
                    throw new Exception("Could not find AppDbContext in injection container");

                if (context.Lgas.Any())
                    return;

                if (!context.CountryStates.Any())
                    await SeedCountryState(context);

                if (!context.Lgas.Any())
                    await SeedLga(context);
            }
        }
    }

    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public AppDbContext()
        {
        }

        public DbSet<CountryState> CountryStates { get; set; }

        public DbSet<Lga> Lgas { get; set; }

        public DbSet<Customer> Customers { get; set; }
    }

    public class CountryState
    {

        public int Id { get; set; }

        [StringLength(50)]
        public string Name { get; set; }
    }

    public class Customer
    {
        public Guid Id { get; set; }

        public Lga Lga { get; set; }
        public int LgaId { get; set; }

        [StringLength(100)]
        public string Email { get; set; }

        [StringLength(100)]
        public string Password { get; set; }

        [StringLength(20)]
        public string PhoneNumber { get; set; }

        public bool IsVerified { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime ModifiedAt { get; set; }
    }

    public class Lga
    {
        public int Id { get; set; }

        public int StateId { get; set; }

        public string Name { get; set; }

        public CountryState State { get; set; }
    }

    public class LGAData
    {
        public int Id { get; set; }
        public string Lga { get; set; }
        public string State { get; set; }
    }

    [Table("OasisD1SOCReport")]
    public class OasisD1SOCReport
    {
        public Guid Id { get; set; }
        public ICollection<OasisD1SOCReportQuestion> Questions { get; set; }
    }

    [Table("OasisD1SOCReportQuestions")]
    public class OasisD1SOCReportQuestion
    {
        public Guid Id { get; set; }

        public Guid OasisD1SOCReportId { get; set; }

        public OasisD1SOCReport OasisD1SOCReport { get; set; }

        public string Name { get; set; }

        public string Answer { get; set; }
    }

    public enum QuestionType
    {
        Moo,
        PlanofCare,
        Generic,
        Gg,
        Jj
    }
    public abstract class Question
    {
        public string Name { get; set; }
        public string Answer { get; set; }
        public string Code { get; set; }
        public QuestionType Type { get; set; }

    }

    public static class H
    {
        public static IDictionary<string, Question> ToOASISDictionary(this List<Question> questions)
        {
            IDictionary<string, Question> questionDictionary = new Dictionary<string, Question>();
            var key = string.Empty;
            if (questions.Any())
            {
                questions.ForEach(question =>
                {
                    if (question.Type == QuestionType.Moo || question.Type == QuestionType.Gg || question.Type == QuestionType.Jj)
                    {
                        key = string.Format("{0}{1}", question.Code, question.Name);
                        if (!questionDictionary.ContainsKey(key))
                        {
                            questionDictionary.Add(key, question);
                        }
                    }
                    else if (question.Type == QuestionType.PlanofCare)
                    {
                        key = string.Format("485{0}", question.Name);
                        if (!questionDictionary.ContainsKey(key))
                        {
                            questionDictionary.Add(key, question);
                        }
                    }
                    else if (question.Type == QuestionType.Generic)
                    {
                        key = string.Format("Generic{0}", question.Name);
                        if (!questionDictionary.ContainsKey(key))
                        {
                            questionDictionary.Add(key, question);
                        }
                    }
                    else
                    {
                        key = string.Format("{0}", question.Name);
                        if (!questionDictionary.ContainsKey(key))
                        {
                            questionDictionary.Add(key, question);
                        }
                    }
                });
            }
            return questionDictionary;
        }
    }
}