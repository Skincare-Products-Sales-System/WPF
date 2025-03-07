﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SPSS.DAL.Models;

[Index("QuestionId", Name = "IX_SkinTypeQuestions")]
public partial class SkinTypeQuestion
{
    [Key]
    [Column("QuestionID")]
    public int QuestionId { get; set; }

    [StringLength(200)]
    public string QuestionText { get; set; }

    [StringLength(300)]
    public string AnswerOptions { get; set; }

    [InverseProperty("Question")]
    public virtual ICollection<UserAnswer> UserAnswers { get; set; } = new List<UserAnswer>();
}