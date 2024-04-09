{/* Footer function */ }
function Footer() {
    return (
        <footer>
            {/* You're able to use javascript directly into tags etc. with { } */ }
            <p>&copy; {new Date().getFullYear()} Website Name</p>
        </footer>
    )
}

{/* To be able to import this function elsewhere */ }
export default Footer