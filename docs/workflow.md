# Workflow

## Proposing spec changes
1. Create a card on the [task board](https://github.com/Azure/bicep/projects/1) in the "To do" column, named something like "{my feature} Proposal"
2. When ready to start work, use "Convert to issue" and move it to the "In progress" column
3. Aim to cover the following when writing your spec:
    * Goals & Non-goals.
    * High level syntax (note, it's fine to start with something hand-wavy, just aim to refine and come up with something concrete post-discussion).
    * Example syntax uses.
4. Move the issue into "In review" and request reviews from the relevant team members.
5. Address comments either by amending the issue or justifying why you don't think it should be adopted.
6. If you're unable to reach a conclusion, reach out to the [Benevolent Dictator](https://github.com/marcre) to make a decision.
7. Move your card to "Done" and start work on the PR to incorporate your proposal.

## Making spec changes
TBD. Notes:
* Ensure grammar and spec are kept up-to-date.
* Ensure you have round-trip tests covering the new syntax.
* Keep the reference templates (links TBD) up-to-date with your new syntax.

## Considerations when proposing changes
Please add to this list with new considerations.

* How does this concept fit stylistically with the existing spec?
* How intuitive will this feel to read and write to programmers and non-programmers?
* What will error recovery look like if the syntax has been partially typed?
* What will IDE suggestions, errors and completions look like within this syntax?