function AdjustFixedHoliday(year, month, day) {
    var d = new Date(year, month, day);

    switch (d.getDay()) {
        case 0:
            d.setDate(d.getDate() + 1);
            break;
        case 6:
            d.setDate(d.getDate() - 1);
            break;
    }
    return d;
}

function getMemorialDay(year) {
    // Set first of last month
    var d = new Date(year, 5, 1);

    // Roll the days backwards until Monday.
    if (d.getDay() !== 1) {
        do {
            d.setDate(d.getDate() - 1);
        } while (d.getDay() !== 1);
    }
    return d;
};

function getLaborDay(year) {
    // Set first of last month
    var d = new Date(year, 8, 1);

    // Roll the days backwards until Monday.
    if (d.getDay() !== 1) {
        do {
            d.setDate(d.getDate() + 1);
        } while (d.getDay() !== 1);
    }
    return d;
};

function getThanksgiving(year) {
    //Get Last day of the month
    var d = new Date(year, 11, 0);
    // Move back a week if needed
    if (d.getDay() < 5) {
        d.setDate(d.getDate() - 7);
    }
    // Roll the days backwards until Thursday.
    if (d.getDay() !== 4) {
        do {
            d.setDate(d.getDate() - 1);
        } while (d.getDay() !== 4);
    }
    return d;
}

function Holidays(year = new Date().getFullYear()) {
    const holidays = [];

    holidays.push({
        name: "New Year's Day",
        date: AdjustFixedHoliday(year, 0, 1)
    });

    holidays.push({
        name: "Memorial Day",
        date: getMemorialDay(year)
    });

    holidays.push({
        name: "Independence Day",
        date: AdjustFixedHoliday(year, 6, 4)
    });

    holidays.push({
        name: "Labor Day",
        date: getLaborDay(year)
    });

    holidays.push({
        name: "Thanskgiving",
        date: getThanksgiving(year)
    });

    holidays.push({
        name: "Christmas Day",
        date: AdjustFixedHoliday(year, 11, 25)
    });

    return holidays;
}