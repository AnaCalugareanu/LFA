from graphviz import Digraph

class FiniteAutomaton:
    def __init__(self, states, alphabet, transition_function, start_state, accept_states):
        self.states = states
        self.alphabet = alphabet
        self.transition_function = transition_function
        self.start_state = start_state
        self.accept_states = accept_states

    def to_regular_grammar(self):
        grammar = {}
        for state in self.states:
            grammar[state] = set()
            for char in self.alphabet:
                if (state, char) in self.transition_function:
                    target_state = self.transition_function[(state, char)]
                    grammar[state].add(char + target_state)
            if state in self.accept_states:
                grammar[state].add('ε')  # ε represents an empty string (epsilon)
        return grammar

    def visualize(self):
        dot = Digraph()
        dot.attr(rankdir='LR', size='8,5')

        # Non-accept states
        for state in self.states - self.accept_states:
            dot.node(state, shape='circle')
        # Accept states
        for state in self.accept_states:
            dot.node(state, shape='doublecircle')

        # Invisible start node
        dot.node('', shape='none')
        dot.edge('', self.start_state)

        # Transitions
        for (src, symbol), dst in self.transition_function.items():
            dot.edge(src, dst, label=symbol)

        return dot

# Define the FA based on your variant
states = {'q0', 'q1', 'q2', 'q3'}
alphabet = {'a', 'b'}
transition_function = {
    ('q0', 'a'): 'q1',
    ('q0', 'a'): 'q2',  # This transition makes the FA non-deterministic
    ('q1', 'b'): 'q1',
    ('q1', 'a'): 'q2',
    ('q2', 'a'): 'q1',
    ('q2', 'b'): 'q3'
}
start_state = 'q0'
accept_states = {'q3'}  # Update this based on your variant

def is_deterministic(fa):
    for state in fa.states:
        seen_transitions = {}
        for char in fa.alphabet:
            if (state, char) in fa.transition_function:
                if char in seen_transitions:
                    return False  # More than one transition for a state and symbol
                seen_transitions[char] = fa.transition_function[(state, char)]
            else:
                # Allow missing transitions for determinism check; they're implicit to a sink state
                continue
    return True

def convert_ndfa_to_dfa(ndfa):
    # Create new DFA
    new_states = set(['q0'])  # Start with the initial state
    new_accept_states = set()
    new_transition_function = {}
    unprocessed_states = [{'q0'}]  # States to process

    while unprocessed_states:
        current_new_state = unprocessed_states.pop()
        for char in ndfa.alphabet:
            next_new_state = set()
            for state in current_new_state:
                if (state, char) in ndfa.transition_function:
                    next_new_state.add(ndfa.transition_function[(state, char)])
            if next_new_state:
                new_state_name = ''.join(sorted(next_new_state))
                new_transition_function[(''.join(sorted(current_new_state)), char)] = new_state_name
                if new_state_name not in new_states:
                    new_states.add(new_state_name)
                    unprocessed_states.append(next_new_state)
                if next_new_state & ndfa.accept_states:
                    new_accept_states.add(new_state_name)

    return FiniteAutomaton(new_states, ndfa.alphabet, new_transition_function, 'q0', new_accept_states)

fa = FiniteAutomaton(states, alphabet, transition_function, start_state, accept_states)
rg = fa.to_regular_grammar()
print("Regular Grammar:")
for left, productions in rg.items():
    for production in productions:
        print(f"{left} -> {production}")

print("The FA is", "deterministic" if is_deterministic(fa) else "non-deterministic")

dfa = convert_ndfa_to_dfa(fa)
print("Converted DFA:")
for state in dfa.states:
    print(f"State: {state}")
    for char in dfa.alphabet:
        if (state, char) in dfa.transition_function:
            print(f" δ({state}, {char}) = {dfa.transition_function[(state, char)]}")

fa_dot = fa.visualize()

# Render the graph to a file and view it
output_path = 'finite_automaton'
fa_dot.render(output_path, view=True, format='png')
