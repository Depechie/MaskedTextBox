MaskedTextBox
=============

Win RT masked text box behavior

I'm trying to create a good masked text box behavior for WinRT!

What I started with is a behavior that can be used on a TextBox. It currently has one dependency property called Mask where you can define simple masks. The only fixed chars that are available for the mask are 9 ( for numeric placeholders ) and # ( for all chars placeholders ). So you could already set up a mask like this: 99.99.###.999

But the behavior still has some flaws!

- Copy paste a large string will not trigger a correct mask formatting
- While deleting and again entering chars, will sometime result in the need to enter the same char twice
