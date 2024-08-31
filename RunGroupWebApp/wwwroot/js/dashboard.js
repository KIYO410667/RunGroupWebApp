
    function showSection(section) {
        document.getElementById('racesSection').style.display = 'none';
    document.getElementById('clubsSection').style.display = 'none';
    document.getElementById('racesTab').classList.remove('active');
    document.getElementById('clubsTab').classList.remove('active');

    if (section === 'races') {
        document.getElementById('racesSection').style.display = 'block';
    document.getElementById('racesTab').classList.add('active');
    }
    else if (section === 'clubs') {
        document.getElementById('clubsSection').style.display = 'block';
    document.getElementById('clubsTab').classList.add('active');
    }
}

    // Default to showing races
    showSection('races');
