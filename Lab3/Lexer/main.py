from enum import Enum

class SymbolType(str, Enum):
    NUMBER = "NUMBER"
    ADD = "ADD"
    SUBTRACT = "SUBTRACT"
    MULTIPLY = "MULTIPLY"
    DIVIDE = "DIVIDE"
    OPEN_BRACE = "OPEN_BRACE"
    CLOSE_BRACE = "CLOSE_BRACE"
    END = "END"
    UNKNOWN = "UNKNOWN"

class Symbol:
    def __init__(self, kind, value=None):
        self.kind = kind
        self.value = value

    def __str__(self):
        return f"Symbol({self.kind}, {self.value})"

    def __repr__(self):
        return self.__str__()

    def equals(self, other):
        return self.kind == other.kind and self.value == other.value

class Analyser:
    def __init__(self, source_code):
        self.source = source_code
        self.pointer = 0
        self.current = self.update_current()

    def update_current(self):
        if self.pointer >= len(self.source):
            return None
        return self.source[self.pointer]

    def move_forward(self):
        self.pointer += 1
        self.current = self.update_current()

    def ignore_space(self):
        while self.current and self.current.isspace():
            self.move_forward()

    def collect_number(self):
        number = ''
        while self.current and self.current.isdigit():
            number += self.current
            self.move_forward()
        return int(number)

    def next_symbol(self):
        while self.current:
            if self.current.isspace():
                self.ignore_space()
                continue

            if self.current.isdigit():
                return Symbol(SymbolType.NUMBER, self.collect_number())

            if self.current == "+":
                self.move_forward()
                return Symbol(SymbolType.ADD)

            if self.current == "-":
                self.move_forward()
                return Symbol(SymbolType.SUBTRACT)

            if self.current == "*":
                self.move_forward()
                return Symbol(SymbolType.MULTIPLY)

            if self.current == "/":
                self.move_forward()
                return Symbol(SymbolType.DIVIDE)

            if self.current == "(":
                self.move_forward()
                return Symbol(SymbolType.OPEN_BRACE)

            if self.current == ")":
                self.move_forward()
                return Symbol(SymbolType.CLOSE_BRACE)

            # For unknown characters
            self.move_forward()
            return Symbol(SymbolType.UNKNOWN, self.current)

        return Symbol(SymbolType.END)

    def extract_symbols(self):
        symbols = []
        while (symbol := self.next_symbol()).kind != SymbolType.END:
            symbols.append(symbol)
        symbols.append(symbol)
        return symbols

# Example
code_to_analyze = "3 + 4 * (2 - 1)"
analyzer = Analyser(code_to_analyze)
extracted_symbols = analyzer.extract_symbols()
for sym in extracted_symbols:
    print(sym)
