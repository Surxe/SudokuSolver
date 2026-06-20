#!/usr/bin/env python3
"""
CSV Parser for Kaggle Sudoku Dataset
Converts Kaggle format CSV to JSON format for the Sudoku Solver project.

Kaggle format: No headers, two columns per row
- Column 1: Initial board state (81 characters, 0 = empty)
- Column 2: Solved board state (81 characters)

Output format: JSON array of puzzles with 9x9 2D arrays
"""

import csv
import json
import sys
import argparse
from pathlib import Path
from typing import List, Dict, Tuple, Optional


def parse_board_string(board_str: str) -> List[List[int]]:
    """
    Convert an 81-character string to a 9x9 2D array.
    
    Args:
        board_str: 81-character string representing the board (0-9)
    
    Returns:
        9x9 2D list of integers
    
    Raises:
        ValueError: If string length is not 81 or contains invalid characters
    """
    if len(board_str) != 81:
        raise ValueError(f"Board string must be 81 characters, got {len(board_str)}")
    
    if not board_str.isdigit():
        raise ValueError(f"Board string must contain only digits 0-9")
    
    board = []
    for i in range(9):
        row_start = i * 9
        row_end = row_start + 9
        row = [int(digit) for digit in board_str[row_start:row_end]]
        board.append(row)
    
    return board


def validate_board(board: List[List[int]]) -> Tuple[bool, Optional[str]]:
    """
    Validate a 9x9 board structure.
    
    Args:
        board: 9x9 2D list of integers
    
    Returns:
        Tuple of (is_valid, error_message)
    """
    if len(board) != 9:
        return False, f"Board must have 9 rows, got {len(board)}"
    
    for i, row in enumerate(board):
        if len(row) != 9:
            return False, f"Row {i} must have 9 columns, got {len(row)}"
        
        for j, cell in enumerate(row):
            if not (0 <= cell <= 9):
                return False, f"Cell ({i},{j}) must be 0-9, got {cell}"
    
    return True, None


def parse_csv(input_path: Path, output_path: Path, include_solved: bool = False) -> int:
    """
    Parse Kaggle CSV file and convert to JSON format.
    
    Args:
        input_path: Path to input CSV file
        output_path: Path to output JSON file
        include_solved: Whether to include solved boards in output
    
    Returns:
        Number of puzzles successfully parsed
    """
    puzzles = []
    puzzle_count = 0
    error_count = 0
    
    with open(input_path, 'r', newline='') as csvfile:
        reader = csv.reader(csvfile)
        
        for row_num, row in enumerate(reader, start=1):
            if len(row) < 1:
                print(f"Warning: Row {row_num} is empty, skipping")
                error_count += 1
                continue
            
            if len(row) < 2:
                print(f"Warning: Row {row_num} has only {len(row)} column(s), expected 2")
                error_count += 1
                continue
            
            initial_str = row[0].strip()
            solved_str = row[1].strip() if len(row) > 1 else None
            
            try:
                initial_board = parse_board_string(initial_str)
                is_valid, error_msg = validate_board(initial_board)
                
                if not is_valid:
                    print(f"Error: Row {row_num} - {error_msg}")
                    error_count += 1
                    continue
                
                puzzle = {
                    "id": puzzle_count + 1,
                    "board": initial_board
                }
                
                if include_solved and solved_str:
                    try:
                        solved_board = parse_board_string(solved_str)
                        is_valid, error_msg = validate_board(solved_board)
                        
                        if is_valid:
                            puzzle["solved_board"] = solved_board
                        else:
                            print(f"Warning: Row {row_num} solved board invalid - {error_msg}")
                    except ValueError as e:
                        print(f"Warning: Row {row_num} solved board parse error - {e}")
                
                puzzles.append(puzzle)
                puzzle_count += 1
                
            except ValueError as e:
                print(f"Error: Row {row_num} - {e}")
                error_count += 1
    
    # Write output JSON
    output_path.parent.mkdir(parents=True, exist_ok=True)
    
    with open(output_path, 'w') as jsonfile:
        json.dump(puzzles, jsonfile, indent=2)
    
    print(f"Successfully parsed {puzzle_count} puzzles")
    print(f"Encountered {error_count} errors")
    print(f"Output written to {output_path}")
    
    return puzzle_count


def main():
    parser = argparse.ArgumentParser(
        description='Convert Kaggle Sudoku CSV to JSON format'
    )
    parser.add_argument(
        'input',
        type=Path,
        help='Input CSV file path'
    )
    parser.add_argument(
        'output',
        type=Path,
        nargs='?',
        default=None,
        help='Output JSON file path (default: data/puzzles.json)'
    )
    parser.add_argument(
        '--include-solved',
        action='store_true',
        help='Include solved boards in output'
    )
    
    args = parser.parse_args()
    
    if not args.input.exists():
        print(f"Error: Input file not found: {args.input}")
        sys.exit(1)
    
    output_path = args.output if args.output else Path('data/puzzles.json')
    
    try:
        parse_csv(args.input, output_path, args.include_solved)
    except Exception as e:
        print(f"Error: {e}")
        sys.exit(1)


if __name__ == '__main__':
    main()
