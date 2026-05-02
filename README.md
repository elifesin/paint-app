# A MINIMALIST PAINT APP WITH WPF 

This project is a desktop drawing application developed in C# using Windows Presentation Foundation (WPF) framework. It provides a clean and intuitive canvas for users to sketch, draw shapes), and add text.
The application's architecture is deeply rooted in modern software design patterns, specifically the **Command Pattern** and **Strategy Pattern**, ensuring a scalable and maintainable codebase.

**Features**

-Freehand Drawing: A versatile Pen tool for free-flowing sketches.

-Shape Creation: Dedicated tools for rendering perfect Rectangles, Circles (Ellipses) and Squares.

-Text Input: A Text tool to directly annotate the canvas via interactive text boxes.

-Undo/Redo System: A robust action history manager allowing users to seamlessly undo mistakes or redo actions.


**DESIGN PATTERNS**

Main logic by using behavorial design patterns is improve extendiility, reduce repeated code and seperate concerns.

*1- Command Pattern*: This pattern is mainly used for undo/redo functionality. It sees every action as an object. 

-ICommand: The base interface defining Execute() and Undo() methods.

-DrawingCommandManager: Manages the undoStack and redoStack, coordinating the
execution and reversal of operations.

-Concrete Commands: Classes like AddShapeCommand, AddTextCommand, and PenDrawingCommand handle the specific logic for adding and removing elements from the WPF Canvas.

*2- Strategy Pattern:* This pattern is used for indirectly alter the object’s behavior at runtime by associating it with different sub-objects which can perform specific sub-tasks in different ways. It handles different drawing tools dynamically. 


