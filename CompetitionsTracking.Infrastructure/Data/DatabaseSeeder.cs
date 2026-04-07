using System;
using System.Collections.Generic;
using System.Linq;
using CompetitionsTracking.Domain.Entities;

namespace CompetitionsTracking.Infrastructure.Data
{
    public static class DatabaseSeeder
    {
        public static void Seed(CompetitionsTrackingDbContext context)
        {
            if (context.Persons.Any())
            {
                return;   
            }

            var mentor = new Person 
            {
                Name = "John", 
                Surname = "Doe", 
                Country = "USA", 
                DateOfBirth = new DateTime(1980, 1, 1).ToUniversalTime(), 
                Gender = Gender.Male,
                Type = "Person"
            };
            
            var mentee = new Person 
            {
                Name = "Jane", 
                Surname = "Smith", 
                Country = "USA", 
                DateOfBirth = new DateTime(2005, 5, 5).ToUniversalTime(), 
                Gender = Gender.Female,
                Type = "Person",
                Mentor = mentor
            };

            var team = new Team
            {
                Name = "Eagles",
                Coach = mentor,
                Members = new List<Person> { mentee },
                Type = "Team"
            };

            var apparatus = new Apparatus { Type = "Hoop" };
            
            var discipline = new Discipline 
            { 
                Type = "Individual Hoop", 
                Apparatus = apparatus 
            };

            var category = new Category 
            { 
                Type = "Seniors", 
                MinAge = 16, 
                MaxAge = 99 
            };

            var competition = new Competition 
            { 
                Title = "National Championship 2026", 
                City = "New York", 
                StartDate = DateTime.UtcNow.AddDays(30), 
                EndDate = DateTime.UtcNow.AddDays(35), 
                Status = CompetitionStatus.Planned 
            };

            var entry = new Entry 
            { 
                Competition = competition, 
                Participant = mentee, 
                Discipline = discipline, 
                Category = category, 
                ApplicationStatus = ApplicationStatus.Accepted, 
                EntryStatus = EntryStatus.Active, 
                SubmittedAt = DateTime.UtcNow 
            };

            var judgePerson = new Person 
            { 
                Name = "Robert", 
                Surname = "Brown", 
                Country = "Canada", 
                DateOfBirth = new DateTime(1975, 3, 3).ToUniversalTime(), 
                Gender = Gender.Male,
                Type = "Person" 
            };

            var judge = new Judge 
            { 
                Person = judgePerson, 
                QualificationLevel = "International" 
            };

            var score = new Score 
            { 
                Entry = entry, 
                Judge = judge, 
                Type = ScoreType.D, 
                ScoreValue = 15.5f 
            };

            var result = new Result 
            { 
                Entry = entry, 
                Place = 1, 
                FinalScore = 15.5f, 
                AwardedMedal = "Gold" 
            };

            var appeal = new Appeal 
            { 
                Result = result, 
                Reason = "Scoring mistake on element 3", 
                Status = AppealStatus.Pending, 
                CreatedAt = DateTime.UtcNow, 
                ResolvedAt = DateTime.UtcNow 
            };

            context.Persons.AddRange(mentor, mentee, judgePerson);
            context.Teams.Add(team);
            context.Apparatuses.Add(apparatus);
            context.Disciplines.Add(discipline);
            context.Categories.Add(category);
            context.Competitions.Add(competition);
            context.Entries.Add(entry);
            context.Judges.Add(judge);
            context.Scores.Add(score);
            context.Results.Add(result);
            context.Appeals.Add(appeal);

            context.SaveChanges();
        }
    }
}
