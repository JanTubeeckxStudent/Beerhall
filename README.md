# Beerhall
Problem 'using xunit' not working -> solution: put content <ItemGroup> test.csproj in main .csproj 
and add following statement under tag PropertyGroup to solve call of two main methods: "<GenerateProgramFile>false</GenerateProgramFile>" 
