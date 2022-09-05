using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Amazon.Textract;
using Amazon.Textract.Model;
using DiscordBot.Domain.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace DiscordBot.Infrastructure.Services;

public class ImageRecognition : IImageRecognition, IDisposable
{
    private readonly IAmazonTextract _textract;

    private const string FEATURE_TYPE_TABLES = "TABLES";
    
    public ImageRecognition(IAmazonTextract textract)
    {
        _textract = textract;
    }
    
    public async Task<string> ConvertImageToCsv(byte[] imageData)
    {
        //var request = new DetectDocumentTextRequest();
        var request = new AnalyzeDocumentRequest();
        request.Document = new Document {
            Bytes = new MemoryStream(imageData)
        };
        request.FeatureTypes = new List<string> { FEATURE_TYPE_TABLES };
        //var result = await this.textract.DetectDocumentTextAsync(request);
        var response = await this._textract.AnalyzeDocumentAsync(request);

        var blocksMap = new Dictionary<string, Block>();
        var tableBlocks = new List<Block>();
        var blocks = response.Blocks;
        foreach (var block in blocks)
        {
            blocksMap.Add(block.Id, block);
            if (block.BlockType == BlockType.TABLE)
            {
                tableBlocks.Add(block);
            }
        }

        if (tableBlocks.Count == 0) throw new InvalidOperationException("No tables found");

        var result = "";
        var tableIndex = 1;
        foreach (var table in tableBlocks)
        {
            result += generateTableCsv(table, blocksMap, tableIndex);
            result += "\n\n";
            tableIndex++;
        }

        return result;
    }

    private string generateTableCsv(Block table, Dictionary<string, Block> blocksMap, int tableIndex)
    {
        var rows = getRowsColumnsMap(table, blocksMap);
        var tableId = "Table_" + tableIndex;

        var csv = $"Table: {tableId}\n\n";
        
        foreach (var row in rows)
        {
            foreach (var col in row.Value)
            {
                csv += "\"" + col.Value.Replace("\"", "\"\"") + "\",";
            }

            csv += "\n";
        }

        csv += "\n\n\n";
        return csv;
    }

    private Dictionary<int, Dictionary<int, string>> getRowsColumnsMap(Block table, Dictionary<string, Block> blocksMap)
    {
        var rows = new Dictionary<int, Dictionary<int, string>>();
        foreach (var relationship in table.Relationships)
        {
            if (relationship.Type == RelationshipType.CHILD)
            {
                foreach (var childId in relationship.Ids)
                {
                    var cell = blocksMap[childId];
                    if (cell.BlockType == BlockType.CELL)
                    {
                        var rowIndex = cell.RowIndex;
                        var colIndex = cell.ColumnIndex;

                        if (!rows.ContainsKey(rowIndex))
                            rows.Add(rowIndex, new Dictionary<int, string>());

                        rows[rowIndex].Add(colIndex, getText(cell, blocksMap));
                    }
                }
            }
        }

        return rows;
    }

    private string getText(Block result, Dictionary<string, Block> blocksMap)
    {
        var text = "";
        if (result.Relationships is { Count: > 0 })
        {
            foreach (var relationship in result.Relationships)
            {
                if (relationship.Type == RelationshipType.CHILD)
                {
                    foreach (var childId in relationship.Ids)
                    {
                        var word = blocksMap[childId];
                        if (word.BlockType == BlockType.WORD)
                        {
                            text += word.Text + " ";
                        }

                        if (word.BlockType == BlockType.SELECTION_ELEMENT)
                        {
                            if (word.SelectionStatus == SelectionStatus.SELECTED)
                            {
                                text += "x ";
                            }
                        }
                    }
                }
            }
        }

        return text;
    }

    /*
    private void Print(List<Block> blocks) {
        blocks.ForEach(x => {
            if(x.BlockType.Equals("LINE")) {
                Console.WriteLine(x.Text);
            }
        });
    }
    
    public void Print(DetectDocumentTextResponse response) {
        if(response != null) {
            this.Print(response.Blocks);
        }
    }
    
    public void Print(List<GetDocumentTextDetectionResponse> response) {
        if(response != null && response.Count > 0) {
            response.ForEach(r => this.Print(r.Blocks));
        }
    }
    */
    
    public List<string> GetLines(DetectDocumentTextResponse result) {
        var lines = new List<string>();
        result.Blocks.FindAll(block => block.BlockType == "LINE").ForEach(block => lines.Add(block.Text));
        return lines;
    }

    public void Dispose()
    {
        _textract?.Dispose();
    }
}