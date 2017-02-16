# LayerGen

Automatically generate Business and Data layers in C# or VB.Net from a SQL Server, Microsoft Access, MySql or SQLite database design.
Download Source Code

This code was [originally published by icemanind](https://www.codeproject.com/Articles/1000660/LayerGen) whose release notes are reproduced below. I change it to entity framework and moved the source to github in order to facilitate ongoing development.

### Introduction

A while back (like 7 years ago), I released a little program called "LayerGen MMX". I had started LayerGen back in 2002 around the time the .NET framework first got released. After LayerGen MMX, I wanted to completely rewrite the program from scratch. I did that and 7 years later, I am proud to present LayerGen 3.5.
If you are unfamiliar with LayerGen, it is an ORM tool that will automatically generate Data Layers and Business Layers based off a database schema. It will generate code in either C# or VB.NET. It currently works with Microsoft SQL Server, versions 2000 up to 2014, Microsoft Access, SQLite or MySql. In addition, the code it generates should work with .NET 2.0 up to the current .NET version. In addition, you can also configure LayerGen dynamically to work with either Stored Procedures or straight SQL text (note: Microsoft Access and SQLite do not support stored procedures).
After using LayerGen for a while, you will soon come to realize that LayerGen's biggest selling point and feature is its ease of use and intuitivness. Compared with similar products, LayerGen makes accessing databases a breeze.

