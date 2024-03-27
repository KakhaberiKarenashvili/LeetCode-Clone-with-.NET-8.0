using informaticsge.Dto;
using informaticsge.models;
using informaticsge.Models;
using Riok.Mapperly.Abstractions;

namespace informaticsge.Mapper;

[Mapper]
public abstract partial class Mapper
{
    public partial MyAccountDTO ToMyAccountDto(User user);

    public partial SolutionsDTO ToSolutionsDto(Solution solution);
    
    

}