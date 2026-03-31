using System.Collections.Generic;
using System.Linq;
using DnDWorldMapEditor.Data;
using DnDWorldMapEditor.Models;
using Microsoft.EntityFrameworkCore;

namespace DnDWorldMapEditor.HelperFunctions;

public class GridSpaceFunctions
{
     public static async void UpdateGridSpacesAfterMapEdit(ApplicationDbContext context, int oldRowsTotal, int oldColumnsTotal, int newRowsTotal, int newColumnsTotal, int worldMapId)
        {
            //ToDo Deleting gridSpaces when row/col count decreases doesn't work, FIX IT!
            //When lowering the number of rows, and col, an extra row is leftover
            int rowDiff = oldRowsTotal - newRowsTotal;
            int columnDiff = oldColumnsTotal - newColumnsTotal;
            bool rowIncrease = false;
            bool columnIncrease = false;
            bool rowDecrease = false;
            bool columnDecrease = false;
            var gridSpacesToDelete =  new List<GridSpace>();
            var gridSpacesToAdd =  new List<GridSpace>();

            if (rowDiff < 0) { rowIncrease = true; }
            else if (rowDiff > 0) { rowDecrease = true; }
            
            if (columnDiff < 0) { columnIncrease = true; }
            else if (columnDiff > 0) { columnDecrease = true; }
            

            //delete grid Spaces if col/rows decrease
            if (columnDecrease)
            {
                var colSpaces = await context.GridSpace.Where(x => 
                    x.WorldMapId == worldMapId && 
                    x.Col > (newColumnsTotal - 1)).ToListAsync();
                foreach (var colSpace in colSpaces)
                {
                    gridSpacesToDelete.Add(colSpace);
                }
            }
            if (rowDecrease)
            {
                var rowSpaces = await context.GridSpace.Where(x => 
                    x.WorldMapId == worldMapId && 
                    x.Row > (newRowsTotal - 1)).ToListAsync();
                foreach (var rowSpace in rowSpaces)
                {
                    gridSpacesToDelete.Add(rowSpace);
                }
            }

            if (gridSpacesToDelete.Count > 0)
            {
                DeleteGridSpaces(context, gridSpacesToDelete);   
            }

            //add grid Spaces if col/rows increase
            if (rowIncrease && columnIncrease)
            {
                for (int i = oldRowsTotal; i < newRowsTotal; i++)
                {
                    for (int j = 0; j < newColumnsTotal; j++)
                    {
                        GridSpace newSpace = new GridSpace(worldMapId, i, j);
                        gridSpacesToAdd.Add(newSpace);
                        
                    }
                }
                for (int i = 0; i < oldRowsTotal; i++)
                {
                    for (int j = oldColumnsTotal; j < newColumnsTotal; j++)
                    {
                        GridSpace newSpace = new GridSpace(worldMapId, i, j);
                        gridSpacesToAdd.Add(newSpace);
                    }
                }
            }
            else if (rowIncrease && !columnIncrease)
            {
                for (int i = oldRowsTotal; i < newRowsTotal; i++)
                {
                    for (int j = 0; j < newColumnsTotal; j++)
                    {
                        GridSpace newSpace = new GridSpace(worldMapId, i, j);
                        gridSpacesToAdd.Add(newSpace);
                        
                    }
                }
            }
            else if (columnIncrease && !rowIncrease)
            {
                for (int i = 0; i < newRowsTotal; i++)
                {
                    for (int j = oldColumnsTotal; j < newColumnsTotal; j++)
                    {
                        GridSpace newSpace = new GridSpace(worldMapId, i, j);
                        gridSpacesToAdd.Add(newSpace);
                        
                    }
                }
            }

            if (gridSpacesToAdd.Count > 0)
            {
                AddGridSpaces(context, gridSpacesToAdd);
            }
            
            
        }

        public static async void CreateGridSpaces(ApplicationDbContext context, WorldMap worldMap)
        {
            int worldMapId = worldMap.Id;
            for (int i = 0; i < worldMap.TotalRows; i++)
            {
                for (int j = 0; j < worldMap.TotalColumns; j++)
                {
                    GridSpace gridSpace = new GridSpace(worldMapId,  i, j);
                    context.GridSpace.Add(gridSpace);
                    await context.SaveChangesAsync();
                }
            }
        }

        public static async void AddGridSpaces(ApplicationDbContext context, List<GridSpace> gridSpacesToAdd)
        {
            foreach (var gridSpace in gridSpacesToAdd)
            {
                context.GridSpace.Add(gridSpace);
                await context.SaveChangesAsync();
            }
        }
        
        public static async void DeleteGridSpaces(ApplicationDbContext context, List<GridSpace> gridSpacesToDelete)
        {
            foreach (var gridSpace in gridSpacesToDelete)
            {
                context.GridSpace.Remove(gridSpace);
                await context.SaveChangesAsync();
            } 
        }
}